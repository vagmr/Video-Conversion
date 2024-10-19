# Video-Conversion

CLI for converting h264 video to h265 video using FFmpeg

## 下载说明

本程序提供两个版本：

1. Framework-dependent（框架依赖版）
   - 文件更小，但需要安装 .NET 6.0 运行时
   - 如果你的电脑已经安装了 .NET 6.0，推荐使用此版本
2. Self-contained（自包含版）
   - 文件较大，但无需安装任何依赖
   - 可以在任何 Windows 电脑上直接运行
   - 如果你不确定是否安装了 .NET 运行时，使用此版本

## 系统要求

- Windows 操作系统
- FFmpeg（需要单独安装，确保 FFmpeg 在系统 PATH 中）

## 安装步骤

1. 从 [Releases](https://github.com/vagmr/video-conversion/releases) 页面下载适合你系统的版本。
2. 移动或复制下载的文件到你想要的目录。
3. 确保已经安装了 FFmpeg 并添加到系统 PATH 中。

## 使用说明

打开命令提示符或 PowerShell，导航到程序所在目录，然后使用以下格式运行程序：

```bash
VideoConverter [输入文件] [选项]
```

## 开发说明
1.clone 项目
```bash
git clone https://github.com/vagmr/video-conversion.git
```
2. 配置.net 6.0 sdk
```bash
dotnet tool install --global dotnet-sdk
```
3. 编辑源代码
4. 编译
```bash
dotnet publish -c Release -p:PublishSingleFile=true --no-self-contained  -o .\build\ .\VideoConverter.csproj
```


### 选项

- `-o, --output [文件路径]`: 指定输出文件路径（可选，默认随机生成）
- `-c, --crf [值]`: 设置视频质量（默认：28，范围：0-51，越低质量越好）
- `-p, --preset [值]`: 设置编码速度（默认：slow）
- `-ac, --audio-codec [值]`: 设置音频编码格式（默认：aac）
- `-r, --resolution [值]`: 设置视频分辨率（例如：720, 480）
- `-e, --encoder [值]`: 设置编码器（默认：nvenc）
- `-h, --help`: 显示帮助信息

#### 编码器预设 (preset) 说明
preset 参数详细说明：
- nvenc 编码器

| 预设       | 编号 | 描述                |
|------------|------|---------------------|
| default    | 0    | 默认值，等同于 p4   |
| slow       | 1    | 高质量，2 次编码 (HQ)|
| medium     | 2    | 中等质量，1 次编码 (HQ)|
| fast       | 3    | 高性能 (HP)         |
| hp         | 4    | 高性能              |
| hq         | 5    | 高质量              |
| bd         | 6    | Blu-ray 兼容性      |
| ll         | 7    | 低延迟编码          |
| llhq       | 8    | 低延迟高质量        |
| llhp       | 9    | 低延迟高性能        |
| lossless   | 10   | 无损编码            |
| losslesshp | 11   | 无损高性能          |
| p1         | 12   | 最快，质量最低      |
| p2         | 13   | 较快，较低质量      |
| p3         | 14   | 快速，低质量        |
| p4         | 15   | 中等，默认          |
| p5         | 16   | 较慢，高质量        |
| p6         | 17   | 更慢，更好质量      |
| p7         | 18   | 最慢，最佳质量      |

- libx265 编码器

| 预设       | 编号 | 描述                |
|------------|------|---------------------|
| ultrafast  | 0    | 最快                  |
| superfast  | 1    | 较快                  |
| veryfast   | 2    | 快速                  |
| faster     | 3    | 更快                  |
| fast       | 4    | 快速                  |
| medium     | 5    | 中等                  |
| slow       | 6    | 较慢                  |
| slower     | 7    | 更慢                  |
| veryslow   | 8    | 最慢                  |

### 示例

转换视频，使用默认设置：

```bash
VideoConverter input.mp4
```

指定输出文件名和 CRF 值：

```bash
VideoConverter input.mp4 -o output.mp4 --crf 24
```

转换视频并更改分辨率：

```bash
VideoConverter input.mp4 --resolution 720 --preset fast
```

## 注意事项

- 确保有足够的磁盘空间来存储转换后的视频。
- 转换过程可能需要一些时间，取决于视频长度和你的计算机性能。
- 如果遇到任何问题，请确保 FFmpeg 已正确安装并添加到系统 PATH 中。

## 常见问题

1. Q: 程序报错说找不到 FFmpeg，该怎么办？
   A: 请确保已经正确安装 FFmpeg 并添加到系统 PATH 中。你可以在命令行中输入 `ffmpeg -version` 来检查是否正确安装。

2. Q: 为什么转换后的文件比原文件大？
   A: h265 编码通常会产生比 h264 更小的文件，但这取决于你设置的 CRF 值和原视频的编码质量。尝试增加 CRF 值（例如从 28 增加到 30）来减小文件大小，但这可能会略微降低视频质量。

## 更新日志

- 添加了批量转换功能
- 添加了 libx265 编码器支持

