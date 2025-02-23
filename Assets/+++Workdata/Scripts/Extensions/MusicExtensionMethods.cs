using UnityEngine;
using System.Collections;

public static class MusicExtensionMethods
{
    /// <summary>
   /// Fades out the current audio clip and fades in the new audio clip
   /// </summary>
   /// <param name="aSource">the audio source</param>
   /// <param name="clip">the clip to play</param>
   /// <param name="duration">the duration to fade</param>
   public static void FadingInOut(this AudioSource aSource, AudioClip clip, float duration)
   {
      aSource.GetComponentInParent<MonoBehaviour>().StartCoroutine(FadeOutAudio(aSource, clip, duration));
   }

   /// <summary>
   /// turn down the volume of the audio source and change the clip after stop playing
   /// goes to fade in coroutine
   /// </summary>
   /// <param name="aSource">the audio source</param>
   /// <param name="clip">the clip to play</param>
   /// <param name="duration">the duration to fade</param>
   /// <returns></returns>
   private static IEnumerator FadeOutAudio(AudioSource aSource, AudioClip clip, float duration)
   {
      //use the stopwatch to be independent to the time set in unity
      float startVolume = aSource.volume; //save the initial volume
      float elapsedTime = 0f; // Track elapsed Time
      System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew(); //start a stopwatch

      
      while (elapsedTime < duration)
      {
         elapsedTime = (float)stopwatch.Elapsed.TotalSeconds; //get elapsed time in seconds
         aSource.volume = Mathf.Lerp(startVolume, 0.001f, elapsedTime / duration); //interpolate volume
         yield return null;
      }

      aSource.volume = 0.001f; //ensure the volume is set to minimum
      aSource.Stop();
      aSource.clip = clip;
      aSource.volume = 1f;
      aSource.GetComponentInParent<MonoBehaviour>().StartCoroutine(FadeInAudio(aSource, duration, startVolume));
   }

   /// <summary>
   /// plays audio and turn up the volume of the audio source
   /// </summary>
   /// <param name="aSource">the audio source</param>
   /// <param name="duration">the duration to fade</param>
   /// <returns></returns>
   private static IEnumerator FadeInAudio(AudioSource aSource, float duration, float endVolume)
   {
      //use the stopwatch to be independent to the time set in unity
      float elapsedTime = 0f; // Track elapsed Time
      System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew(); //start a stopwatch
      aSource.Play();

      while (elapsedTime < duration)
      {
         elapsedTime = (float)stopwatch.Elapsed.TotalSeconds; //get elapsed time in seconds
         aSource.volume = Mathf.Lerp(0.001f, endVolume, elapsedTime / duration); //interpolate volume

         yield return null;
      }

      aSource.volume = endVolume;
   }
}
