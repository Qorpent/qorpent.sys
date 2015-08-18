namespace Qorpent.Config {
    public interface IConfigurationLoader {
        IConfigProvider Load(ConfigurationOptions options);
    }
}