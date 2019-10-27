using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Disc
    {       
        public List<DiscProperty.PropertyType> discProperties;
        public DiscMould.MouldName mouldName;
        
       // public Color discColor; //Legacy
        //public Color stampColor; //Legacy        
               
        public string colorsName;
        public int stampVariant;
        
        public Disc()
        {
           
        }

        public Disc(DiscMould mould, int level)
        {

        }

        public Disc(DiscMould mould, DiscProperty property, DiscColor color)
        {          
            mouldName = mould.mouldName;
            colorsName = color.name;
            discProperties = new List<DiscProperty.PropertyType>
            {
               property.type
            };                   
        }

        public Disc(DiscMould mould, List<DiscProperty.PropertyType> properties, DiscColor color)
        {            
            mouldName = mould.mouldName;
            colorsName = color.name;
            discProperties = properties;                      
        }
    }
}
