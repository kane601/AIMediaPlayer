﻿using System.IO;
using System.Windows;
using FlyleafLib;
using FlyleafLib.MediaPlayer;

namespace AIMediaPlayer.Services;

public static class FlyleafLoader
{
    public static void StartEngine()
    {
        EngineConfig engineConfig = DefaultEngineConfig();

        // Load Player's Config
        if (File.Exists(App.EngineConfigPath))
        {
            try
            {
                engineConfig = EngineConfig.Load(App.EngineConfigPath, AppConfig.GetJsonSerializerOptions());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load EngineConfig from {Path.GetFileName(App.EngineConfigPath)}, Please review the settings or delete the config file. Error details are recorded in {Path.GetFileName(App.CrashLogPath)}.");
                try
                {
                    File.WriteAllText(App.CrashLogPath, "EngineConfig Loading Error: " + ex);
                }
                catch
                {
                    // ignored
                }

                Application.Current.Shutdown();
            }
        }

        Engine.Start(engineConfig);
    }

    public static Player CreateFlyleafPlayer()
    {
        Config? config = null;
        bool useConfig = false;

        // Load Player's Config
        if (File.Exists(App.PlayerConfigPath))
        {
            try
            {
                config = Config.Load(App.PlayerConfigPath, AppConfig.GetJsonSerializerOptions());
                useConfig = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load PlayerConfig from {Path.GetFileName(App.PlayerConfigPath)}, Please review the settings or delete the config file. Error details are recorded in {Path.GetFileName(App.CrashLogPath)}.");
                try
                {
                    File.WriteAllText(App.CrashLogPath, "PlayerConfig Loading Error: " + ex);
                }
                catch
                {
                    // ignored
                }

                Application.Current.Shutdown();
            }
        }

        config ??= DefaultConfig();
        Player player = new(config);

        if (!useConfig)
        {
            // Initialize default key bindings for custom keys for new config.
            foreach (var binding in AppActions.DefaultCustomActionsMap())
            {
                config.Player.KeyBindings.Keys.Add(binding);
            }
        }

        return player;
    }

    public static EngineConfig DefaultEngineConfig()
    {
        EngineConfig engineConfig = new()
        {
#if DEBUG
            PluginsPath = @":Plugins\bin\Plugins.NET9",
#else
            PluginsPath = ":Plugins",
#endif
            FFmpegPath = ":FFmpeg",
            FFmpegHLSLiveSeek = true,
            UIRefresh = true,
#if DEBUG
            LogOutput = ":debug",
            LogLevel = LogLevel.Debug,
            FFmpegLogLevel = Flyleaf.FFmpeg.LogLevel.Warn,
#endif
        };

        return engineConfig;
    }

    private static Config DefaultConfig()
    {
        Config config = new();
        config.Demuxer.FormatOptToUnderlying =
            true; // Mainly for HLS to pass the original query which might includes session keys
        config.Audio.FiltersEnabled = true; // To allow embedded atempo filter for speed
        config.Video.GPUAdapter = ""; // Set it empty so it will include it when we save it
        config.Subtitles.SearchLocal = true;

        // TODO: L: Allow customization in settings
        // Give top most priority to English
        config.Audio.Languages =
            config.Audio.Languages.Take(config.Audio.Languages.Count - 1)
                .Prepend(config.Audio.Languages.Last()).ToList();
        config.Subtitles.Languages =
            config.Subtitles.Languages.Take(config.Subtitles.Languages.Count - 1)
                .Prepend(config.Subtitles.Languages.Last()).ToList();
        return config;
    }
}
