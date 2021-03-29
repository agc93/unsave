using System.Collections.Generic;
using UnSave.Serialization;
using UnSave.Types;

namespace UnSave
{
    public class SaveSerializerBuilder
    {
        private List<IUnrealPropertySerializer> _propertySerializers { get; } = new List<IUnrealPropertySerializer>();

        private List<IUnrealCollectionPropertySerializer> _collectionSerializers { get; } =
            new List<IUnrealCollectionPropertySerializer>();
        public SaveSerializerBuilder AddDefaultSerializers()
        {
            var structSerializers = new List<IUnrealStructSerializer> {new UEGuidStructProperty()};
            _propertySerializers.AddRange(new List<IUnrealPropertySerializer>
            {
                new ArrayPropertySerializer(),
                new BoolPropertySerializer(),
                new BytePropertySerializer(),
                new ColorPropertySerializer(),
                new DateTimePropertySerializer(),
                new EnumPropertySerializer(),
                new FloatPropertySerializer(),
                new IntPropertySerializer(),
                new RotatorPropertySerializer(),
                new StringPropertySerializer(),
                new TextPropertySerializer(),
                new VectorPropertySerializer(),
                new UEStructSerializer(structSerializers)
            });
            _collectionSerializers.Add(new UEStructSerializer(structSerializers));
            return this;
        }

        public SaveSerializerBuilder AddSerializer(IUnrealPropertySerializer serializer)
        {
            _propertySerializers.Insert(0, serializer);
            return this;
        }

        public SaveSerializerBuilder AddCollectionSerializer(IUnrealCollectionPropertySerializer serializer)
        {
            _collectionSerializers.Insert(0, serializer);
            return this;
        }

        public SaveSerializer Build()
        {
            var propSer = new PropertySerializer(_propertySerializers, _collectionSerializers);
            var ser = new SaveSerializer(propSer);
            return ser;
        }
        
    }
}