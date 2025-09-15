using System;

[Serializable]
public class UserVectorData
{
    public double[] Vector;
}

[Serializable]
public class RecommendationResponse
{
    public string[] recommendations;
}

[Serializable]
public class CosPointRe
{
    public uint[] OtherUser;
    public uint Target;
}

[Serializable]
public class NearUserResponse
{
    public uint[] nearUsers;
}