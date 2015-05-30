namespace qorpent.v2.security.authentication {
    public interface ITokenEncryptor {
        Token Decrypt(string srctoken);
        string Encrypt(Token token);

    }
}