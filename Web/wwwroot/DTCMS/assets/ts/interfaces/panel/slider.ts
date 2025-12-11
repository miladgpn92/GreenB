interface SliderModel {
  id: string | number;
  title: string;
  picUrl: string;
  link: string;
  isActive: boolean;
}

interface SliderResponse extends SliderModel {
  createDate: string;
  applicationUserName: string;
  applicationUserFamily: string;
}

export { SliderResponse, SliderModel };
