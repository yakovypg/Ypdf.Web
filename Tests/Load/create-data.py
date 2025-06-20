import os
import time
import itertools

from PIL import Image
from reportlab.pdfgen import canvas

JPEG_QUALITY = 100
IMG_SIZE = (10000, 10000)
OUTPUT_PDF_DOCUMENTS_COUNT = 3
OUTPUT_PDF_DOCUMENT_NAME_PREFIX = 'file'

def get_next_color_generator():
    colors = [
        (  0,   0,   0),    # black
        (255,   0,   0),    # red
        (  0, 255,   0),    # green
        (  0,   0, 255),    # blue
        (255, 255,   0),    # yellow
        (255,   0, 255),    # magenta
        (  0, 255, 255),    # cyan
        (255, 255, 255),    # white
    ]

    colors_cycle = itertools.cycle(colors)

    def get_next_color():
        return next(colors_cycle)

    return get_next_color

def generate_solid_image(path, size, color, quality):
    image = Image.new('RGB', size, color)
    image.save(path, format='JPEG', quality=quality)

def build_pdf(output_path, image_paths, page_size=IMG_SIZE):
    pdf_canvas = canvas.Canvas(output_path, pagesize=page_size)
    width, height = page_size

    for image in image_paths:
        pdf_canvas.drawImage(image, 0, 0, width=width, height=height)
        pdf_canvas.showPage()

    pdf_canvas.save()

def create_pdf(output_path, pages_count=1, next_color_generator=None):
    image_paths = []

    next_color = (
        next_color_generator
        if next_color_generator is not None
        else get_next_color_generator()
    )

    for i in range(pages_count):
        timestamp = time.perf_counter_ns()
        image_path = f'__tmp_{timestamp}_img{i}.jpg'
        image_fill = next_color()

        generate_solid_image(
            image_path,
            IMG_SIZE,
            color=image_fill,
            quality=JPEG_QUALITY
        )

        image_paths.append(image_path)

    build_pdf(output_path, image_paths)

    for image_path in image_paths:
        os.remove(image_path)

def main():
    next_color = get_next_color_generator()

    for i in range(1, OUTPUT_PDF_DOCUMENTS_COUNT + 1):
        output_path = f'{OUTPUT_PDF_DOCUMENT_NAME_PREFIX}{i}.pdf'
        create_pdf(output_path, pages_count=i, next_color_generator=next_color)

if __name__ == '__main__':
    main()
