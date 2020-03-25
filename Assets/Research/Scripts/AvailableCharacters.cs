using System.Collections.Generic;
using MoreMountains.TopDownEngine;

namespace Research.Scripts
{
    public class AvailableCharacters
    {
        private readonly List<Character> _availableMlCharacters;

        private readonly List<Character> _allCharacters;
        
        private System.Random _random;

        public AvailableCharacters()
        {
            _random = new System.Random();
            _availableMlCharacters = new List<Character>();
            _allCharacters = new List<Character>();
        }

        public void Add(Character newCharacter)
        {
            _availableMlCharacters.Add(newCharacter);
            _allCharacters.Add(newCharacter);
        }
        
        public Character PopRandomCharacter()
        {
            var index = _random.Next(0, _availableMlCharacters.Count);
            var character = _availableMlCharacters[index];
            _availableMlCharacters.Remove(character);
            
            character.gameObject.SetActive(true);
            return character;
        }

        public void ReturnCharacter(Character character)
        {
            if (_allCharacters.Contains(character))
            {
                _availableMlCharacters.Add(character);
            }
        }
    }
}
