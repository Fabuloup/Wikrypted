namespace wikrypted;

public class Krypter
{
    private static Krypter _instance;
    private static readonly object _lock = new object();
    public string Key { get; private set; }

    private Krypter() { }

    public static Krypter Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Krypter();
                }
                return _instance;
            }
        }
    }

    public void SetKey()
    {
        Key = Guid.NewGuid().ToString();
    }

    public string Encode(string input)
    {
        if (string.IsNullOrEmpty(Key))
        {
            SetKey();
        }

        var encodedChars = input.Select(c => c == '\n' ? c : (char)(c + PerlinNoise(Key.GetHashCode() + (int)(c - '0')) * 10)).ToArray();
        return new string(encodedChars);
    }

    private double PerlinNoise(int x)
    {
        // Implémentation de Perlin Noise
        x = (x << 13) ^ x;
        return (1 - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
    }
}