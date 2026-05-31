import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewInit
} from '@angular/core';

import { BrowserMultiFormatReader }
from '@zxing/browser';
import { HeaderEscanearMesa } from '../components/header-escanear-mesa/header-escanear-mesa';

@Component({
  selector: 'app-escanear-mesa',
  imports: [HeaderEscanearMesa],
  templateUrl: './escanear-mesa.html',
  styleUrls: ['./escanear-mesa.css']
})
export class ScanQr
implements AfterViewInit {

  @ViewChild('video')
  video!: ElementRef<HTMLVideoElement>;

  scanner =
    new BrowserMultiFormatReader();

  async ngAfterViewInit() {

    const devices =
      await BrowserMultiFormatReader
      .listVideoInputDevices();

    const selectedDevice =
      devices[0];

    this.scanner.decodeFromVideoDevice(

      selectedDevice.deviceId,

      this.video.nativeElement,

      (result) => {

        if (result) {

          console.log(
            result.getText()
          );

        }

      }

    );

  }

  cerrarScanner() {
  console.log('cerrar scanner');

  // ejemplo: volver atrás
  window.history.back();
  
}

}