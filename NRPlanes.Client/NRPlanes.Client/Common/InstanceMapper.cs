using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRPlanes.Client.Common
{
    public class InstanceMapper
    {
        private readonly PlanesGame _game;

        private readonly CoordinatesTransformer _coordinatesTransformer;

        private readonly Dictionary<Type, ConstructorInfo> _classMapDictionary;

        public InstanceMapper(PlanesGame game, CoordinatesTransformer coordinatesTransformer)
        {
            _game = game;

            _coordinatesTransformer = coordinatesTransformer;

            _classMapDictionary = new Dictionary<Type, ConstructorInfo>();
        }

        public void AddMapping(Type modelType, Type xnaType)
        {
            var constructors = xnaType.GetConstructors();

            if (constructors.Length != 1)
                throw new Exception("Xna UI classes must have ONLY ONE public constructor");

            var constructor = constructors[0];

            _classMapDictionary.Add(modelType, constructor);
        }

        public MyDrawableGameComponent CreateInstance(object modelObject)
        {
            var modelObjType = modelObject.GetType();

            if (!_classMapDictionary.ContainsKey(modelObjType))
            {
                throw new Exception("This model type not matched");
            }

            var constructor = _classMapDictionary[modelObjType];

            var xnaObj = (MyDrawableGameComponent) constructor.Invoke(new object[]
                                                                          {
                                                                              _game,
                                                                              modelObject,
                                                                              _coordinatesTransformer
                                                                          });

            return xnaObj;
        }
    }
}