using R2API;
using System.IO;

namespace AtsukoSeekerVoiceover.Modules
{
    public static class SoundBanks
    {
        private static bool initialized = false;
        public static string SoundBankDirectory
        {
            get
            {
                return Files.assemblyDir;
            }
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            //UnityEngine.Debug.Log("AssemblyDir: " + Files.assemblyDir);
            using (Stream manifestResourceStream = new FileStream(SoundBankDirectory +"\\AtsukoSeekerSoundbank.bnk", FileMode.Open))
            {

                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
    }
}
