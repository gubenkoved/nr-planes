using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRPlanes.Client.Common
{
    /// <summary>
    /// Every game world essence (GameObject-derived, Equipment-derived classes) that should be drawn have to be mapped to some MyDrawableGameComponent-derived class
    /// </summary>
    public class InstanceMapper
    {
        private readonly PlanesGame m_game;
        private readonly CoordinatesTransformer m_coordinatesTransformer;
        private readonly Dictionary<Type, Tuple<ConstructorInfo, object[]>> m_classMapDictionary;

        public InstanceMapper(PlanesGame game, CoordinatesTransformer coordinatesTransformer)
        {
            m_game = game;
            m_coordinatesTransformer = coordinatesTransformer;
            m_classMapDictionary = new Dictionary<Type, Tuple<ConstructorInfo, object[]>>();
        }

        /// <summary>
        /// Register model class -> drawable class mapping.
        /// <para>Every drawable class constructor will be called with next parameters:</para>
        /// <para>1. Planes game reference</para>
        /// <para>2. Model object reference</para>
        /// <para>3. Coordinates transformer reference</para>
        /// <para>All other parameters (optional)</para>
        /// </summary>
        public void AddMapping(Type modelType, Type xnaType, object[] additionalCtorParams = null)
        {
            if (additionalCtorParams == null)
                additionalCtorParams = new object[0];

            ConstructorInfo[] constructors = xnaType.GetConstructors();

            if (constructors.Length != 1)
                throw new Exception("Xna UI classes must have ONLY ONE public constructor");

            ConstructorInfo constructor = constructors[0];

            m_classMapDictionary.Add(modelType, new Tuple<ConstructorInfo, object[]>(constructor, additionalCtorParams));
        }

        public MyDrawableGameComponent CreateInstance(object modelObject)
        {
            Type modelObjType = modelObject.GetType();

            if (!m_classMapDictionary.ContainsKey(modelObjType))
            {
                throw new Exception("This model type not matched");
            }

            ConstructorInfo constructor = m_classMapDictionary[modelObjType].Item1;
            object[] additionalParams = m_classMapDictionary[modelObjType].Item2;

            object[] parameters = new object[3 + additionalParams.Length];
            parameters[0] = m_game;
            parameters[1] = modelObject;
            parameters[2] = m_coordinatesTransformer;

            for (int i = 0; i < additionalParams.Length; i++)
            {
                parameters[i + 3] = additionalParams[i];
            }

            return (MyDrawableGameComponent)constructor.Invoke(parameters);
        }
    }
}