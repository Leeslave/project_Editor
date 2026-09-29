using System;
using UnityEngine;

namespace EditorGame.Documents
{
    /// <summary>원본을 수정하지 않고 사용하는 Desktop90 UI 이미지 참조다.</summary>
    [Serializable]
    public sealed class DocumentDesktopSkin
    {
        public Sprite Raised;
        public Sprite Pressed;
        public Sprite Selected;
        public Sprite Recessed;
        public Sprite ScrollTrack;
        public Sprite Toolbar;
        public Sprite Close;
        public Sprite Minimize;
        public Sprite WindowIcon;
    }
}
