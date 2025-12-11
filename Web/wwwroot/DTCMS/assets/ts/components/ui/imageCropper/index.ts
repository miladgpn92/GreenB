import Cropper from "cropperjs";
import Compressor from "compressorjs";
import { base64ToFile, getRoundedCanvas } from "../../../utils";

class ImageCropper {
  maxSize: number;
  imageheight: number;
  imagequality: number;
  cropperRatio: number | null;
  roundCrop: boolean;
  imagePreview: HTMLElement;
  fileInput: File;
  imageElement?: HTMLImageElement;
  previousZoomLevel: number;
  cropper: Cropper;
  isCompressed: boolean;

  rangeInput: HTMLInputElement;

  constructor(
    fileInput: File,
    imagePreview: HTMLElement,
    maxSize = 80000,
    imageHeight: number = 1024,
    imageQuality: number = 0.6,
    cropperRatio: number | null = 16 / 9,
    rangeInput: HTMLInputElement,
    imageElement: HTMLImageElement,
    roundCrop: boolean,
    isCompressed: boolean
  ) {
    this.fileInput = fileInput;
    this.imagePreview = imagePreview;
    this.maxSize = maxSize;
    this.imageheight = imageHeight;
    this.imagequality = imageQuality;
    this.cropperRatio = cropperRatio;
    this.rangeInput = rangeInput;
    this.imageElement = imageElement ? imageElement : null;
    this.roundCrop = roundCrop;
    this.isCompressed = isCompressed;
    this.previousZoomLevel = 0;
  }
  initCropper = () => {
    const reader = new FileReader();
    this.imagePreview.classList.remove("hidden");
    let prev = this.imagePreview.querySelector(
      "#preview" + this.imagePreview.id
    );
    if (prev) prev.remove();
    reader.onload = (e) => {
      const img = new Image();
      img.onload = () => {
        img.className = "block max-w-[390px] min-h-[284px] max-w-full";
        img.id = "preview" + this.imagePreview.id;
        this.imagePreview.prepend(img);
        // Initialize the cropper with aspect ratio NaN for free cropping
        this.cropper = new Cropper(img, {
          aspectRatio: this.cropperRatio !== 0 ? this.cropperRatio : NaN, // Set the aspect ratio to 1:1 for a circular crop
          viewMode: 1, // Show only the cropped area
          dragMode: "move", // Enable dragging to move the image within the cropper
          //   cropBoxResizable: false, // Disable resizing the crop box
          //   cropBoxMovable: false, // Disable moving the crop box
          //   autoCropArea: 1, // Show the entire image within the crop box
          ready: () => {
            this.cropper.zoom(-1);
          },
        });
      };
      img.src = e.target.result as string;
    };

    reader.readAsDataURL(this.fileInput);
  };

  handleCrop = async (callBack: Function) => {
    try {
      let canvas: HTMLCanvasElement;
      if (this.roundCrop) {
        canvas = getRoundedCanvas(this.cropper.getCroppedCanvas());
      } else {
        canvas = this.cropper.getCroppedCanvas();
      }

      const croppedImageBase64 = canvas.toDataURL(this.fileInput.type);

      if (this.imageElement) this.imageElement.src = croppedImageBase64;
      const imageFile = base64ToFile(croppedImageBase64, this.fileInput.name);
      let compressedImage: File;
      if (this.isCompressed === true) {
        await ImageCropper.CompressImage(
          imageFile,
          this.imagequality,
          this.imageheight,
          this.maxSize,
          (item) => {
            compressedImage = item;
            this.cropper.clear();
            this.cropper.reset();
            this.cropper.destroy();
            this.imagePreview
              .querySelector("#preview" + this.imagePreview.id)
              .remove();
            this.imagePreview.classList.add("hidden");
            callBack(item);
          }
        );
      } else {
        debugger
        await ImageCropper.CompressImage(
          imageFile,
          0.6,
          null,
          imageFile.size,
          (item) => {
            compressedImage = item;
            this.cropper.clear();
            this.cropper.reset();
            this.cropper.destroy();
            this.imagePreview
              .querySelector("#preview" + this.imagePreview.id)
              .remove();
            this.imagePreview.classList.add("hidden");
            callBack(compressedImage);
          }
        );
      }
    } catch (error) {
      console.log(error);
    }
  };

  static CompressImage = (
    uploadedFile: File,
    imagequality: number = 0.6,
    imageheight: number = 1024,
    maxSize: number = 160000,
    callBack?: Function
  ): void => {
    
      new Compressor(uploadedFile, {
        quality: imagequality,
        maxHeight: imageheight,
        convertSize: maxSize,
        success(result) {
          if (result.size > maxSize && imagequality !== 0.1) {
            imagequality = imagequality > 0.1 ? imagequality - 0.1 : 0.01;
            ImageCropper.CompressImage(
              result as File,
              parseFloat(imagequality.toFixed(1)),
              imageheight,
              maxSize,
              callBack
            );
          } else {
            let converted = new File([result], result["name"], {
              type: uploadedFile.type,
            });
            callBack(converted);
          }
        },
        error(err) {
          console.log(err);
        },
      });
    
  };

  handleZoom = () => {
    const zoomValue = parseFloat((event.target as HTMLInputElement).value);
    const diff = parseFloat((zoomValue - this.previousZoomLevel).toFixed(2));
    this.cropper.zoom(diff);
    // Update the previousZoom value
    this.previousZoomLevel = zoomValue;
  };
  setCompressStatus = () => {
    this.isCompressed = !this.isCompressed;
  };
  destroyInstance = () => {
    this.cropper.destroy();
  };
}

export default ImageCropper;
