# Using Docker For ML Agents (Experimental)

We are currently offering an experimental solution for people who'd like to do training or inference using docker. This setup currently forces both python and Unity to rely on _only_ the CPU for computation purposes. So we don't support environments such as [GridWorld](Example-Environments.md#gridworld) which use visual observations for training.

## Setup

- Install [docker](https://www.docker.com/community-edition#/download) if you don't have it setup on your machine. 

- Since Docker runs a container in an environment that is isolated from the host machine, we will be using a mounted directory, e.g. `unity-volume` in your host machine in order to share data, e.g. the Unity executable, curriculum files and tensorflow graph.


## Usage

- Docker typically runs a container sharing a (linux) kernel with the host machine, this means that the Unity environment has to be built for the linux platform. Please select the architecture to be `x86_64` and choose the build to be `headless` (_this is important because we are running it in a container that doesn't have graphics drivers installed_). Save the generated environment in the directory to be mounted (e.g. `unity-volume`). This means your local directory will contain `unity-volume/environment-name.x86_64`


- Build the docker container by running `docker build  -t <image_name>` in the source directory. Replace `<image_name>` by the name of the image that you want to use, e.g. `balance.ball.v0.1`.

- Run the container, for e.g.:
```

docker run --mount type=bind,source="$(pwd)"/unity-volume,target=/unity-volume balance.ball.v0.1:latest environment-name --docker-target-name=unity-volume --train --run-id=103
```

**Note** The docker target volume name, `unity-volume` must be passed to ML-Agents as an argument using the `--docker-target-name` option. The output will be stored in mounted directory. 


