Aimmy is a universal AI-Based Aim Alignment Mechanism developed by BabyHamsta, MarsQQ, and Taylor to make gaming more accessible for users who have difficulty aiming.
Aimmy also provides an easy to use user-interface, a wide set of features and customizability options which makes Aimmy a great option for anyone who wants to use and tailor an Aim Alignment Mechanism for a specific game without having to code.

Aimmy is 100% free to use. This means no ads, no key system, and no paywalled features. Aimmy is not, and will never be for sale for the end user, and is considered a source-available product, **not open source** as we actively discourage other developers from making commercial forks of Aimmy.

Please do not confuse Aimmy as an open-source project, we are not, and we have never been one.

Want to connect with us? Join our [Discord Server](https://discord.gg/aimmy)

If you want to share Aimmy with your friends use our [website!](https://aimmy.dev/)

# Disclaimer
This is a fork of [Aimmy](https://github.com/Babyhamsta/Aimmy/), if any problems ask us on [discord](discord.gg/aimmy).
## What is CUDA
> **What's CUDA?**

```Cuda is pretty much just the better version of "DirectML" and uses Nvidia's GPU power to make it more smoother and faster```

> **What's TensorRT?**

```Pretty much an add-on for Cuda. While it does make your gameplay smoother and faster, it's a double edge sword by making your models loading time drastically slower for 1st time instances```

> **What's DirectML?**

```Think of it as a mid lvl AI that relies on your GPU to work good```

> **How does the AI work?**

```Using the imported models (pictures), it will then scan the game as you play and look for players that match the models (pictures)```
## Setup
- Download and Install the x64 version of [.NET Runtime 8.0.X.X](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.2-windows-x64-installer)
- Download and Install the x64 version of [.NET Runtime 7.0.X.X](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.20-windows-x64-installer)
- Download and Install the x64 version of [Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe)
- Download Aimmy from [Releases](https://github.com/TaylorIsBlue/Aimmy-CUDA/releases) (Make sure it's the Aimmy zip that says **Prepacked CUDA** and not Source zip)
- Extract and run totallynotaimmyv2.exe
- Go to the troubleshooting section if you have issues.

## Setup (troubleshooting)
- Download and Install the x64 version of [.NET Runtime 8.0.X.X](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.2-windows-x64-installer)
- Download and Install the x64 version of [.NET Runtime 7.0.X.X](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.20-windows-x64-installer)
- Download and Install the x64 version of [Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe)
- Download Aimmy from [Releases](https://github.com/TaylorIsBlue/Aimmy-CUDA/releases) (Make sure it's the Aimmy zip and not Source zip)
- **Get [cuDNN 9.x](https://developer.nvidia.com/cudnn-downloads) and [CUDA 12.x](https://developer.nvidia.com/cuda-downloads?target_os=Windows&target_arch=x86_64)**
- Extract the Aimmy.zip file
- Run Aimmy.exe
- Choose your Model and Enjoy :)

### Troubleshooting CUDA
Sometimes, when you load a model the application closes in an exception, this could mean:
1. Your cuda installation is wrong. Check your PATH (env variables) for your Cuda installation and your cuDNN.
2. Download and Install CUDA and cuDNN of [CUDA 12.x](https://developer.nvidia.com/cuda-downloads) and [cuDNN 9.x](https://developer.nvidia.com/cudnn-downloads)
3. Otherwise, make a ticket in our [discord server](discord.gg/aimmy)
