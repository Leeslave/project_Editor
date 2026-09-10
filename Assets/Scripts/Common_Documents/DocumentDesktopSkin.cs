using System;
using UnityEngine;

namespace EditorGame.Documents
{
    /// <summary>Scene-owned references to the installed Desktop90 UI artwork; vendor assets remain unmodified.</summary>
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
