using System;
using FileContainer;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore : IDisposable
    {
        const string SECTION_SETTINGS        = "___settings___";
        const int    SECTION_SETTINGS_LENGTH = 128;

        [NotNull] public string                 Path      { get; }
        [NotNull] public PagedContainerAbstract Container { get; }

        public CDTUnit Unit       { get; private set; }
        public bool    Compressed { get; private set; }

        /// <summary> ColumnStore container </summary>
        /// <param name="unit">
        /// Range function.
        /// All column data will be splitted to sections by range.
        /// Can be set ONLY for new commonPath data. Will read from container for existing data.
        /// </param>
        /// <param name="compressed">
        /// true for use gzip for compression
        /// Can be set ONLY for new commonPath data. Will read from container for existing data.
        /// </param>
        /// <param name="commonPath">prefix for ALL columns (writing/reading) in PersistentColumnStore instance. For example "/Cars/12312" </param>
        public PersistentColumnStore([NotNull] PagedContainerAbstract container, CDTUnit unit = CDTUnit.Month, bool compressed = false, string commonPath = null)
        {
            Container  = container ?? throw new ArgumentException("Can't be null", nameof(container));
            Unit       = unit;
            Compressed = compressed;
            Path       = string.IsNullOrEmpty(commonPath) ? "" : commonPath.Trim('/');

            initSettings(unit, compressed);
        }

        void initSettings(CDTUnit unit, bool compressed)
        {
            var settingsSectionName = string.Join(Path, SECTION_SETTINGS);
            var configData          = Container[settingsSectionName];
            if (configData == null)
            {
                configData                     = new byte[SECTION_SETTINGS_LENGTH];
                configData[0]                  = (byte) unit;
                configData[1]                  = (byte) (compressed ? 1 : 0);
                Container[settingsSectionName] = configData;
            }

            Unit       = (CDTUnit) configData[0];
            Compressed = configData[1] > 0;
        }

        public void Dispose() => Container.Dispose();
    }
}