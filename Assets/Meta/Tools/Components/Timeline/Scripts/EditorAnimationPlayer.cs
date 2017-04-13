using UnityEngine;
using Meta.Tools;
using System.Collections.Generic;
using System;

public static class EditorAnimationPlayer
{
    private static List<TimeLineClip> pausedClips = new List<TimeLineClip>();
	private static List<TimeLineClip> playingClips = new List<TimeLineClip>();

	internal static void PauseClip(TimeLineClip pausingClip, float timing, float speed)
    {
		TimeLineClip foundedClip = playingClips.Find(clip => clip.Go == pausingClip.Go);
		if (foundedClip != null) {
			foundedClip.time = (timing - foundedClip.keyframe.Timing)*foundedClip.Speed;

			if (foundedClip.LoopClip)
			{
				foundedClip.time = foundedClip.time % foundedClip.Clip.length;
			}
			playingClips.Remove (foundedClip);
			pausedClips.Add (foundedClip);
		}
    }


    internal static void SampleClip(TimeLineClip currentClip, float timing, float speed)
    {
		TimeLineClip foundedClip = playingClips.Find(clip => clip.Go == currentClip.Go);
		if (foundedClip != null) {
			
		} 
		else {
			playingClips.Add (currentClip);
		}	

		foundedClip = pausedClips.Find(clip => clip.Go == currentClip.Go);
		if (foundedClip != null) {
			currentClip.offset += foundedClip.time;
			pausedClips.Remove (foundedClip);
		}

        currentClip.SampleClip(timing, speed);
    }

	internal static void RemoveClip(TimeLineClip clip, bool mode){
		if(mode){
			if(playingClips.Contains(clip)){
			playingClips.Remove(clip);
			}
		}
		else{
			if(pausedClips.Contains(clip)){
				pausedClips.Remove(clip);
			}
		}
		clip.offset = 0;
	}
		
}