using System.Collections.Generic;
using UnityEngine;

namespace Research.Common.SpriteSensor
{
    public class SpriteId
    {
        private readonly Dictionary<string, int> _mapper;
        private int _counter;

        private static SpriteId _instance;
        public static SpriteId Instance => _instance ?? (_instance = new SpriteId());

        private SpriteId()
        {
            _mapper = new Dictionary<string, int>();
            var animIds = new []{
                "Damage", 
                "DashParticle", 
                "Dead", 
                "Falling", 
                "Idle", 
                "Run", 
                "SwordIdle",
                "SwordSlash1",
                "SwordSlash2",
                "SwordSlash3",
                "KoalaDamage",
                "KoalaDead"
            };
                
            AddIds(animIds);
        }

        private void AddIds(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                _mapper[id] = _counter++;
            }
        }

        private int[] GetId(string name)
        {
            var split = name.Split('_');
            
            var indexAvailable = split.Length == 3;
            var index = indexAvailable? int.Parse(split[2]) : 0;
            var animIndex = indexAvailable ? 1 : 0;
            var anim =  _mapper[split[animIndex]];
            var results = new [] {anim, index};
            return results;
        }

        public int Length = 2;

        public int[] GetId(SpriteRenderer spriteRenderer)
        {
            return GetId(spriteRenderer.sprite.name);
        }
    }
}
