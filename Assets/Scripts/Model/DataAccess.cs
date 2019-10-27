//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class DataAccess: MonoBehaviour
//    {
//        [SerializeField]
//        private DiscMould[] discMoulds;
//        [SerializeField]
//        private DiscProperty[] discProperties;
//      //  public static DataAccess instance;

//        public Dictionary<DiscMould.DiscType, string> discTypeName = new Dictionary<DiscMould.DiscType, string>()
//        {
//            { DiscMould.DiscType.PuttApproach, "Putt & Approach" },
//            { DiscMould.DiscType.Midrange, "Midrange" },
//            { DiscMould.DiscType.Driver, "Driver" },
//        };

//        private void Awake()
//        {
//            if (instance != null)
//            {
//                Destroy(gameObject);
//            }
//            else
//            {
//                instance = this;
//            }
//            DontDestroyOnLoad(gameObject);
//        }

//        public DiscMould GetMould(DiscMould.MouldName mouldName)
//        {
//            var mould = discMoulds.FirstOrDefault(i => i.mouldName == mouldName);
//            return mould;
//        }

//        public DiscProperty GetProperty(DiscProperty.PropertyType type)
//        {
//            var prop = discProperties.FirstOrDefault(i => i.type == type);
//            return prop;
//        }

//        public DiscProperty GetRandomProperty()
//        {
//            var prop = discProperties[Random.Range(0, discProperties.Length)];
//            return prop;
//        }
//    }
//}
