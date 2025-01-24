using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Audio;
using Utilities.Encoding.Wav;

public class MicRecorder : MonoBehaviour
{
    // Path where we'll save the WAV file (desktop or persistentDataPath)
    private string outputPath;
    
    // Streams WAV data as we record
    private MemoryStream wavData;

    private CancellationTokenSource cancelSource;
    private bool isRecording;

    private void Start()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // For example, store the file in Application.persistentDataPath:
        outputPath = Path.Combine(desktopPath, "MyRecording.wav");
        Debug.Log($"WAV will be saved to: {outputPath}");
    }

    public void BeginRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Already recording!");
            return;
        }

        // Create a new memory stream each time we start fresh
        wavData = new MemoryStream();
        cancelSource = new CancellationTokenSource();

        // Start capturing audio
        // The BufferCallback is invoked whenever new WAV data is available
        // 24000 sample rate is an example; use a rate your mic supports
        RecordingManager.StartRecordingStream<WavEncoder>(BufferCallback, 24000, cancelSource.Token);

        isRecording = true;
        Debug.Log("Started recording microphone to WAV + memory stream...");
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Not recording, nothing to stop.");
            return;
        }

        // Stop the recording manager
        cancelSource?.Cancel();
        isRecording = false;

        // After we stop, wavData should contain a valid WAV file structure
        SaveWavFile();
    }

    // Called whenever new WAV-encoded audio arrives (in chunks)
    private async Task BufferCallback(ReadOnlyMemory<byte> buffer)
    {
        if (!isRecording || wavData == null)
            return;

        // Write the chunk into our memory stream
        try
        {
            // If you also want to send to Realtime, do it here before/after writing to memory:
            // await session.SendAsync(new InputAudioBufferAppendRequest(buffer), ...);

            // Then store locally
            await wavData.WriteAsync(buffer, default);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing WAV data: {e}");
        }
    }

    private void SaveWavFile()
    {
        try
        {
            // Flush the memory (not strictly necessary in some cases, but good practice)
            wavData.Flush();

            // Convert to byte[] and write to disk
            File.WriteAllBytes(outputPath, wavData.ToArray());

            Debug.Log($"WAV file saved successfully at: {outputPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save WAV file: {e.Message}");
        }
        finally
        {
            // Cleanup
            wavData.Dispose();
            wavData = null;
        }
    }

    // Optional: convenience to test quickly
    private void Update()
    {
        // Press 'R' to start recording, 'S' to stop
        if (Input.GetKeyDown(KeyCode.R))
            BeginRecording();
        if (Input.GetKeyDown(KeyCode.S))
            StopRecording();
    }
}
