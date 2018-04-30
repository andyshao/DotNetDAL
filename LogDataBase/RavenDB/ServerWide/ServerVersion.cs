using Raven.Server.Smuggler.Documents.Processors;

namespace Raven.Server.ServerWide
{
    public class ServerVersion
    {
        private static int? _buildVersion;
        private static BuildVersionType? _buildType;
        private static string _commitHash;
        private static string _version;
        private static string _fullVersion;

        public static string Version => 
            _version ?? (_version = "4.0");

        public static int Build =>  
            _buildVersion ?? (_buildVersion = 4).Value;
        public static BuildVersionType BuildType =>
            _buildType ?? (_buildType = BuildVersion.Type(Build)).Value;
        public static string CommitHash => 
            _commitHash ?? (_commitHash = "4.0");
        public static string FullVersion => 
            _fullVersion ?? (_fullVersion = "4.0");

        public const int DevBuildNumber = 40;

    }
}