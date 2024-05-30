namespace ECommerceProject.Core;

public class Resp<T>
{
    public Dictionary<string, string[]> Errors { get; private set; }

    // public int CustomErrorCode { get; set; }

    public T Data { get; set; }

    public void AddError(string key, params string[] errors)
    {
        if (Errors == null)
            Errors = new Dictionary<string, string[]>();

        Errors.Add(key, errors);
    }
}