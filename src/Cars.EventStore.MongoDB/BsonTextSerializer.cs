﻿using System;
using Cars.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Cars.EventStore.MongoDB
{
    public class BsonTextSerializer : ITextSerializer
    {
        public string Serialize(object @object)
        {
            var bsonDoc = BsonDocumentWrapper.Create(@object);

            var ser = bsonDoc.ToJson(new JsonWriterSettings
            { 
                OutputMode = JsonOutputMode.Strict
            });

            return ser;
        }

        public object Deserialize(string textSerialized, string type)
        {
            var doc = BsonDocument.Parse(textSerialized);

            var obj = BsonSerializer.Deserialize(doc, Type.GetType(type));

            return obj;
        }

        public T Deserialize<T>(string textSerialized)
        {
            var doc = BsonDocument.Parse(textSerialized);

            if (doc.TryGetValue("_v", out var subDocument))
            {
                doc = subDocument.AsBsonDocument;
            }

            var obj = BsonSerializer.Deserialize<T>(doc);

            return obj;
        }
    }
}
