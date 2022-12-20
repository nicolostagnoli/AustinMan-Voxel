from PIL import Image, ImageOps
import pathlib

def padding(img, expected_size):
    desired_size = expected_size
    delta_width = desired_size - img.size[0]
    delta_height = desired_size - img.size[1]
    pad_width = delta_width // 2
    pad_height = delta_height // 2
    padding = (pad_width, pad_height, delta_width - pad_width, delta_height - pad_height)
    return ImageOps.expand(img, padding)


def resize_with_padding(img, expected_size):
    img.thumbnail((expected_size[0], expected_size[1]))
    # print(img.size)
    delta_width = expected_size[0] - img.size[0]
    delta_height = expected_size[1] - img.size[1]
    pad_width = delta_width // 2
    pad_height = delta_height // 2
    padding = (pad_width, pad_height, delta_width - pad_width, delta_height - pad_height)
    return ImageOps.expand(img, padding)


if __name__ == "__main__":
    for i in range(0, int(1876/4)):
        y = i*4 + 1
        img = Image.open(str(pathlib.Path(__file__).parent.resolve()) + "\\" + str(y).zfill(4) + ".png")
        img = resize_with_padding(img, (171, 171))
        img.save(str(pathlib.Path(__file__).parent.resolve()) + "\\" + str(y).zfill(4) + ".png")

