using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WhackAMole.Data
{
    /// <summary>
    /// Contain all data which determines a Mole's: behaviour, look, gameplay stats
    /// </summary>
    [CreateAssetMenu(fileName = "Mole", menuName = "WhackAMole/Character", order = 100)]
    [Serializable]
    public class MoleDataObject : ScriptableObject
    {
        [SerializeField] private MoleData _data;
        public MoleData Data => _data;
    }


    [Serializable]
    public class MoleData
    {
        [SerializeField] private int _maxHP = 1;
        [SerializeField] private int _score = 100;
        [SerializeField] private float _minAppearTime = 0.4f;
        [SerializeField] private float _maxAppearTime = 1.5f;
        [Space]
        [SerializeField] private Sprite _activeVisual;
        [SerializeField] private Sprite _defeatVisual;
        [SerializeField] private Sprite _escapeVisual;


        public int MaxHP => _maxHP;
        public int Score => _score;
        public float MinAppearTime => _minAppearTime;
        public float MaxAppearTime => _maxAppearTime;

        public Sprite ActiveVisual => _activeVisual;
        public Sprite DefeatVisual => _defeatVisual;
        public Sprite EscapeVisual => _escapeVisual;
    }
}