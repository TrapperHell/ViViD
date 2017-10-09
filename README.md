# [Vi]meo [Vi]deo [D]ownloader
A Vimeo video downloader proof-of-concept...

## Usage Instructions

1. Open the executable
2. Paste the Vimeo video link and hit Enter
3. App will fail miserably, crash and burn... It didn't? What do you mean, it didn't?! It's supposed to fail. Well in that case, see step #3.i
    1. Video will be downloaded in one of way ways, depending on which one is possible.

**Note:** Using the favoured approach, the video and audio streams will be downloaded as separate stream files. If you want them combined, specify an FFmpeg bin path in the app.config prior to doing the above.

### Disclaimer

This software is provided as-is, as a proof-of-concept of the ability to find, parse, download and join video (and audio) segments from Vimeo. Note that this is provided for educational purposes through the analysis of code - rather than for actual use.

Note that this is a quick and dirty approach. As such look at the code for an idea as to how this is achieved rather than for the merit of the code itself. That being said, feel free to contribute and fix as you see fit.

Code assumes that the best stream quality should be downloaded. If you do not prefer this approach, it's time to get your hands dirty. Simply change the stream ordering to something yoou would prefer instead.

It is very probable that the code is dysfunctional by the time you see this. Due to the nature of the project - a lot of dependencies and assumptions are made on third-party content / services outside of my control.
