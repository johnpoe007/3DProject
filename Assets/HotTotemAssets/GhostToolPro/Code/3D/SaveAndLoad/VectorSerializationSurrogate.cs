using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace GhostToolPro {
	sealed class Vector2SerializationSurrogate : ISerializationSurrogate
	{

	    // Serialize the Employee object to save the object�s name and address fields.
	    public void GetObjectData(System.Object obj,
	        SerializationInfo info, StreamingContext context)
	    {
	        Vector2 vec = (Vector2)obj;
	        info.AddValue("x", vec.x);
	        info.AddValue("y", vec.y);
	    }

	    // Deserialize the Employee object to set the object�s name and address fields.
	    public System.Object SetObjectData(System.Object obj,
	        SerializationInfo info, StreamingContext context,
	        ISurrogateSelector selector)
	    {

	        Vector2 vec = (Vector2)obj;
	        vec = new Vector2((float)info.GetDecimal("x"), (float)info.GetDecimal("y"));
	        return vec;
	    }
	}
	sealed class Vector3SerializationSurrogate : ISerializationSurrogate
	{

	    // Serialize the Employee object to save the object�s name and address fields.
	    public void GetObjectData(System.Object obj,
	        SerializationInfo info, StreamingContext context)
	    {
	        Vector3 vec = (Vector3)obj;
	        info.AddValue("x", vec.x);
	        info.AddValue("y", vec.y);
	        info.AddValue("z", vec.z);
	    }

	    // Deserialize the Employee object to set the object�s name and address fields.
	    public System.Object SetObjectData(System.Object obj,
	        SerializationInfo info, StreamingContext context,
	        ISurrogateSelector selector)
	    {

	        Vector3 vec = (Vector3)obj;
	        vec = new Vector3((float)info.GetDecimal("x"), (float)info.GetDecimal("y"), (float)info.GetDecimal("z"));
	        return vec;
	    }
	}
	sealed class Vector4SerializationSurrogate : ISerializationSurrogate
{

    // Serialize the Employee object to save the object�s name and address fields.
    public void GetObjectData(System.Object obj,
        SerializationInfo info, StreamingContext context)
    {
        Vector4 vec = (Vector4)obj;
        info.AddValue("x", vec.x);
        info.AddValue("y", vec.y);
        info.AddValue("z", vec.z);
        info.AddValue("w", vec.w);
    }

    // Deserialize the Employee object to set the object�s name and address fields.
    public System.Object SetObjectData(System.Object obj,
        SerializationInfo info, StreamingContext context,
        ISurrogateSelector selector)
    {

        Vector4 vec = (Vector4)obj;
        vec = new Vector4((float)info.GetDecimal("x"), (float)info.GetDecimal("y"), (float)info.GetDecimal("z"), (float)info.GetDecimal("w"));
        return vec;
    }
}
}