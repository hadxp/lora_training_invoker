#!/bin/bash
cd ./../diffusion-pipe_framepack

source venv/bin/activate

#NCCL_P2P_DISABLE="1" NCCL_IB_DISABLE="1" deepspeed --num_gpus=1 train.py --deepspeed --config ./../diffusion-pipe-config/main_framepack_video.toml
NCCL_P2P_DISABLE="1" NCCL_IB_DISABLE="1" deepspeed --num_gpus=1 train.py --deepspeed --config ./../diffusion-pipe-config/main_framepack_img.toml
