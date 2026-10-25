using ItemChanger;
using ItemChanger.Internal;
using System;
using UnityEngine;

namespace SpellStaller
{
    [Serializable]
    public class EmbeddedSprite : ISprite
    {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(EmbeddedSprite).Assembly, "SpellStaller.Resources.");

        public string key;
        public EmbeddedSprite(string key)
        {
            this.key = key;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

}
