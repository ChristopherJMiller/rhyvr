using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WWUtils.Audio;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

public class SongManager : MonoBehaviour {

    static ZipFile zf = null;
    static long songFileSize = 0;

    static Stream[] ExtractZipFile(string archiveFilenameIn) {
        FileStream fs = File.OpenRead(archiveFilenameIn);
        zf = new ZipFile(fs);
        Stream[] files = new Stream[2];
        int i = 0;
        foreach (ZipEntry zipEntry in zf) {
            if (!zipEntry.IsFile) {
                continue;			// Ignore directories
            }
            songFileSize = zipEntry.Size;
            files[i] = zf.GetInputStream(zipEntry);
            i++;
        }
        return files;
    }

    public static Task<SongPair> Load(string path)
    {
        Stream[] songFileEntries = ExtractZipFile(path);
        using (StreamReader reader = new StreamReader(songFileEntries[0]))
        {
            Song song =  JsonUtility.FromJson<Song>(reader.ReadToEnd());
            using (StreamReader songFileReader = new StreamReader(songFileEntries[1]))
            using (MemoryStream memStream = new MemoryStream((int)songFileSize))
            {
                Debug.Log(songFileSize);
                var buffer = new byte[512];
                var bytesRead = default(int);
                while ((bytesRead = songFileReader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    memStream.Write(buffer, 0, bytesRead);
                byte[] wavData = memStream.ToArray();
                WAV wavFile = new WAV(wavData);
                Debug.Log(wavFile.ToString());
                AudioClip wav = AudioClip.Create("clip", wavFile.SampleCount, wavFile.ChannelCount, wavFile.Frequency, false);
                float[] data = new float[wavFile.SampleCount * wavFile.ChannelCount];
                int internalLeft = 0;
                int internalRight = 0;
                for(int i = 0; i < wavFile.SampleCount * wavFile.ChannelCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        data[i] = wavFile.LeftChannel[internalLeft];
                        internalLeft++;
                    } else
                    {
                        data[i] = wavFile.RightChannel[internalRight];
                        internalRight++;
                    }
                }
                wav.SetData(data, 0);
                if (zf != null) {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
                return Task.FromResult(new SongPair(song, wav));
            }
        }
    }
}
