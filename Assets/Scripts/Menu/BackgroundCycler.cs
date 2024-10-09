using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class BackgroundCycler : MonoBehaviour
{
    [SerializeField] private Image dynamicBackgroundImage; // UI Image to display backgrounds
    [SerializeField] private float fadeDuration = 5f; // Time it takes to fade in and out
    [SerializeField] private float displayTime = 5f; // Time each image is displayed before fading out
    [SerializeField] private float secondsBetweenImages = 10f; // Time to wait between fading out one image and starting to fade in the next
    [SerializeField] private string imagesPath = "Assets/icons/MenuIcons/BackgroundCycle"; // Path to images folder

    private List<Sprite> backgroundSprites = new List<Sprite>();
    private int currentImageIndex = 0;

private void Start()
{
    // Ensure that the values are set as desired before starting
    fadeDuration = 5f;
    displayTime = 5f;
    secondsBetweenImages = 10f;

    LoadBackgroundImages();

    if (backgroundSprites.Count == 0)
    {
        Debug.LogError("No images found in the specified path.");
        return;
    }

    // Start the cycling process
    StartCoroutine(CycleImages());
}


    private void LoadBackgroundImages()
    {
        // Load all image files from the specified path
        var imageFiles = Directory.GetFiles(imagesPath, "*.png");

        foreach (var filePath in imageFiles)
        {
            var data = File.ReadAllBytes(filePath);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(data);

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            backgroundSprites.Add(sprite);
        }

        Debug.Log($"{backgroundSprites.Count} images loaded for the background cycle.");
    }

    private IEnumerator CycleImages()
    {
        while (true)
        {
            // Set the current image
            dynamicBackgroundImage.sprite = backgroundSprites[currentImageIndex];
            yield return StartCoroutine(FadeIn());

            // Wait for the display time before fading out
            yield return new WaitForSeconds(displayTime);

            // Fade out the current image
            yield return StartCoroutine(FadeOut());

            // Wait for the specified time before starting to fade in the next image
            yield return new WaitForSeconds(secondsBetweenImages);

            // Move to the next image, loop back if necessary
            currentImageIndex = (currentImageIndex + 1) % backgroundSprites.Count;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = dynamicBackgroundImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            dynamicBackgroundImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = dynamicBackgroundImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            dynamicBackgroundImage.color = color;
            yield return null;
        }
    }
}
