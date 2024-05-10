using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WhackAMole.Data
{
    /// <summary>
    /// Contain all data which determines a Game's settings
    /// </summary>
    [CreateAssetMenu(fileName = "GameSetting", menuName = "WhackAMole/GameSettings", order = 10)]
    [Serializable]
    public class GameSettingsDataObject : ScriptableObject
    {
        [SerializeField] private GameSettingsData _data;
        public GameSettingsData Data => _data;
    }


    [Serializable]
    public class GameSettingsData
    {
        [Header("General")]
        [SerializeField, Range(1, 10)] private int _scoreMultiplier = 1;
        [SerializeField, Range(1, 20)] private int _maxSpawnActiveCount = 3;
        [SerializeField, Range(20, 300)] private float _playSessionDurationSec = 120f;
        [SerializeField] private MoleDataObject _defaultMole;

        [Header("Special Mole")]
        [SerializeField] private MoleDataObject _specialMole;
        [SerializeField, Range(0, 1)] private float _specialMoleAppearChance;


        public int ScoreMultiplier => _scoreMultiplier;
        public int MaxSpawnActiveCount => _maxSpawnActiveCount;
        public float PlaySessionDurationSec => _playSessionDurationSec;

        public MoleDataObject GetMole()
        {
            if (_specialMoleAppearChance > 0 &&  UnityEngine.Random.value <= _specialMoleAppearChance)
            {
                return _specialMole;
            }
            else
            {
                return _defaultMole;
            }
        }
    }
}