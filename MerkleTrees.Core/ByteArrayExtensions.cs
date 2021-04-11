namespace MerkleTrees.Core
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4; // extracts the high nibble of a byte

                // b - 10
                // is < 0 for values b < 10, which will become a decimal digit
                // is >= 0 for values b > 10, which will become a letter from A to F.
                //
                // Using i >> 31 on a signed 32 bit integer extracts the sign, thanks to sign extension. It will be -1 for i < 0 and 0 for i >= 0. 
                //
                // =>  (b-10)>>31 will be 0 for letters and -1 for digits.
                //
                // Looking at the case for letters, the last summand becomes 0, and b is in the range 10 to 15. We want to map it to a(97) to f(102), 
                // which implies adding 87 ('a'-10).
                //
                // Looking at the case for digits, we want to adapt the last summand so it maps b from the range 0 to 9 to the range 0(48) to 9(57). 
                // This means it needs to become -39 ('0' - 87). Now we could just multiply with 39.But since - 1 is represented by all bits being 1, 
                // we can instead use &-39 since(0 & -39) == 0 and(-1 & -39) == -39.
                c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));

                b = bytes[i] & 0xF; // extracts the low nibble of a byte
                c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
            }
            return new string(c);
        }
    }
}
