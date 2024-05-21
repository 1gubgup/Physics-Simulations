﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chemix
{
    public class GameManager : Singleton<GameManager>
    {

        [System.Serializable]
        public class ExperimentalScene
        {
            public List<ObjectInfo> objectInfos;
            public List<CodeInfo> codeInfos;
        }

        [System.Serializable]
        public class ObjectInfo
        {
            public string type;
            public string name;
            public string className;
            public Vector3 pos;
            public Vector3 rot;
            public Vector3 scale;
            public int material;
            public bool tracing;
        }

        [System.Serializable]
        public class CodeInfo
        {
            public string codeName;
            public string codeText;
        }

        public ExperimentalScene GetExperimentalScene()
        {
            return GM.GM_Core.instance.experimentalScene;
        }
        ///////////////fen ge///////////////
        /// <summary>
        /// InstrumentInfo contains the type, transform, and stattus of instrument
        /// </summary>
        [System.Serializable]
        public class InstrumentInfo
        {
            public string type;
            public Vector3 position;
            public Quaternion quaternion;
            public string formula;
            public float mass;
        }

        [System.Serializable]
        public class TextInfo
        {
            public Vector3 position;
            public Color color;
            public float size;
        }
        
        [System.Serializable]
        public struct LightInfo
        {
            public Color color;
            public float intensity;
            public Vector3 position;
            public Quaternion rotation;
        }

        [System.Serializable]
        public class EnvironmentInfo
        {
            public float cameraAngle;
            public float cameraHeight;
            public float cameraFOV;
            public bool useRoom;
            public LightInfo[] lightInfo;
        }

        /// <summary>
        /// ExperimentalSetup contains the experimental setup which user created
        /// </summary>
        [System.Serializable]
        public class ExperimentalSetup
        {
            public List<InstrumentInfo> instrumentInfos;
            public TaskFlow taskFlow;
            public TextInfo title;
            public TextInfo detail;
			public Questionnaire.Questionnaire questionnaire;
            public EnvironmentInfo envInfo;
        }

        protected override void Awake()
        {
            base.Awake();

            // setup type2instrument
            if (GM.GM_Core.instance)
            {
                foreach (var instrument in GM.GM_Core.instance.instrumentListAsset.instruments)
                {
                    type2instrument.Add(instrument.type, instrument);
                }
            }
        }

        public enum Formula
        {
            Fe,
            KMnO4,
            HCl,
            H2SO4,
            H2O,
        }

        [System.Serializable]
        public class FormulaInfo
        {
            public string name;
            public Formula formula;
        }

        public static List<FormulaInfo> GetAllFormula()
        {
            List<FormulaInfo> formulaInfos = new List<FormulaInfo>();
            string[] formulaNames = System.Enum.GetNames(typeof(Formula));
            for (int i = 0; i < formulaNames.Length; i++)
            {
                FormulaInfo formulaInfo = new FormulaInfo();
                formulaInfo.formula = (Formula)i;
                formulaInfo.name = formulaNames[i];
                formulaInfos.Add(formulaInfo);
            }
            return formulaInfos;
        }

        public ExperimentalSetup GetExperimentalSetup()
        {
            return GM.GM_Core.instance.experimentalSetup;
        }

        public InstrumentsListAsset.Instrument GetInstrumentByType(string type)
        {
            if (type2instrument.ContainsKey(type))
            {
                return type2instrument[type];
            }
            Debug.LogErrorFormat("GameManager: no instrument for type {0}", type);
            return null;
        }

        private Dictionary<string, InstrumentsListAsset.Instrument> type2instrument = new Dictionary<string, InstrumentsListAsset.Instrument>();
    }
}