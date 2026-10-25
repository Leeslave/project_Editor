"""Validate live story references and the chapter-2 causal/branching contract.

Run from the repository root: python Tools/StoryValidation/validate_story.py
This checks data and routing; it is not a substitute for Unity play testing.
"""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
STREAMING = ROOT / "Assets/StreamingAssets"
CHAT = STREAMING / "ChatData"


def read(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def paragraphs(path):
    return read(path)["chatList"]["$values"]


def scene(day, prefix):
    return next(path for path in (CHAT / f"day {day}").glob("*.json") if path.name.startswith(prefix + " "))


def end(path):
    item = paragraphs(path)[-1]
    return item["action"], item["actionParam"]


def main():
    days = [read(STREAMING / f"DayData/dailyData{index}.json") for index in range(14)]
    active = set()
    registered = set(re.findall(r'Register\("([A-Z]+)"',
        (ROOT / "Assets/Scripts/GameSystem/ActionHandler.cs").read_text(encoding="utf-8-sig")))
    count = 0
    for day, data in enumerate(days):
        assert data["$type"] == "DailyData, Assembly-CSharp"
        for phase, time in enumerate(data["dayTimes"]["$values"]):
            for npc in time["npc"]["$values"]:
                assert len(npc["onAwake"]["$values"]) >= len(npc["chat"]["$values"])
                assert npc["positions"]["$values"] and npc["anchor"]["$values"]
                assert npc["objectType"] != "Price", (day, phase)
                for reference in npc["chat"]["$values"]:
                    path = CHAT / reference.lstrip("/\\")
                    assert path.is_file(), path
                    active.add(path)
                    for item in paragraphs(path):
                        for key in ["action", "reaction"]:
                            action = item.get(key)
                            assert not action or action == "none" or action in registered, (path, action)
                        if item.get("action") == "NEXTTIME":
                            assert int(item["actionParam"]) == phase + 1, (path, phase)
                        if item.get("action") == "EXAM":
                            assert (day, phase) == (12, 3)
                        if item.get("action") == "POSCHANGE":
                            world, position = re.split(r"[, ]+", item["actionParam"].strip())
                            index = ["Street", "Bar", "Cafe", "Restaurant", "Temple", "Hallway", "Office", "Office3", "Interrogate"].index(world)
                            assert not any(v["location"] == index and v["position"] == int(position)
                                           for v in time["block"]["$values"]), (path, "Blocked scene transition")
                        if item.get("action") in ["NEXTDAY", "DAYCHANGE"]:
                            target = day + 1 if item["action"] == "NEXTDAY" else int(item["actionParam"])
                            assert 0 <= target < len(days), (path, target)

    for path in active:
        items = paragraphs(path)
        assert read(path)["$type"] == "Dialogue, Assembly-CSharp"
        for item in items:
            count += 1
            assert len(item["characters"]["$values"]) == 4, path
            for character in item["characters"]["$values"]:
                name = character["fileName"]
                assert "Price" not in (name or ""), path
                if name:
                    assert any((ROOT / "Assets/Resources/Chat/Character").glob(name + ".*")), (path, name)
            background = item.get("background")
            if background and background != "none":
                assert any((ROOT / "Assets/Resources/Chat/Background").glob(background + ".*")), (path, background)
            text = item.get("text", "")
            assert "게임플레이" not in text and "QA 테스트" not in text, (path, text)
            assert "프라이스" not in text and "헤이탄톤" not in text, (path, text)
            if path.parent.name != "day 13":
                assert "혁명단" not in text and "마이클" not in text, (path, text)
            for choice in item.get("choiceList", {}).get("$values", []):
                action = choice.get("reaction")
                assert not action or action == "none" or action in registered
                if action == "JUMP":
                    assert 0 <= int(choice["reactionParam"]) < len(items)

    for day in [1, 2, 3, 7, 8, 9, 10, 11, 12]:
        morning = days[day]["dayTimes"]["$values"][0]
        actions = [item for npc in morning["npc"]["$values"] for ref in npc["chat"]["$values"]
                   for item in paragraphs(CHAT / ref.lstrip("/\\"))]
        assert any(item.get("action") == "NEXTTIME" and item.get("actionParam") == "1" for item in actions), day

    interrogation = scene(12, "[12-3]")
    text = "\n".join(item["text"] for item in paragraphs(interrogation))
    beats = ["홀어머니를 모신다라", "식사를 함께하신 적", "동행해 주십시오", "가까웠다는 증언", "보이드캄프 검사"]
    positions = [text.index(beat) for beat in beats]
    assert positions == sorted(positions), "Threat -> testimony -> arrest -> examination"
    assert end(interrogation) == ("EXAM", "")
    assert end(scene(12, "[12-4]")) == ("NEXTDAY", "")
    assert end(scene(12, "[12-5]")) == ("EXIT", "")
    assert end(scene(13, "[13-H]")) == ("EXIT", "")
    assert not any(item.get("action") in ["NEXTDAY", "DAYCHANGE"] for item in paragraphs(scene(12, "[12-5]")))
    assert days[12]["dayTimes"]["$values"][4]["npc"]["$values"][0]["chat"]["$values"][0].endswith(scene(12, "[12-4]").name)
    assert days[12]["dayTimes"]["$values"][5]["npc"]["$values"][0]["chat"]["$values"][0].endswith(scene(12, "[12-5]").name)
    assert [(work["code"], work["stage"]) for work in days[12]["workList"]["$values"]] == [
        ("ADFGVX", 12), ("Maze", 9), ("VoightKampff", 0)]
    assert days[12]["workList"]["$values"][-1]["availableFromTime"] == 3
    assert days[13]["workList"]["$values"] == []

    final = "\n".join(item["text"] for item in paragraphs(scene(13, "[13-H]")))
    for required in ["내가 그 일을 이끌고 있어", "클로버가 혼자 자료를 가져가려 한 건 나도 몰랐어",
                     "내가 책임져야 할 일", "아직 자네를 용서한 건 아니야", "승낙한 것도 아니네"]:
        assert required in final
    for path in [scene(7, "[7-H]"), scene(12, "[12-4]"), scene(12, "[12-5]"), scene(13, "[13-H]")]:
        assert Path(str(path) + ".meta").is_file()
    # These are source material for chapter 3, not removed or reactivated scenes.
    for day, prefix in [(7, "[7-1]"), (8, "[8-0]"), (8, "[8-5]"), (8, "[8-6]")]:
        assert scene(day, prefix) not in active

    print(f"PASS: 14 daily schedules, {len(active)} referenced dialogue files, {count} unique active paragraphs")
    print("PASS: references, type containers, actions, choices, resources, Price deferral, causal order, success/failure routing")
    print("NOTE: contradictory or unknown calendar years remain unchanged; see Docs/Story/IMPLEMENTATION.md")


if __name__ == "__main__":
    main()
