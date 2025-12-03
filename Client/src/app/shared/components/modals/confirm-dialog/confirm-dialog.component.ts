import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-confirm-dialog',
  standalone: false,
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent {
  title: string = 'Confrim';
  message: string = '';
  btnOkText: string = 'Yes';
  btnCancelText: string = 'No';
  public result: Subject<boolean> = new Subject<boolean>();
  constructor(public bsModalRef: BsModalRef) {}
  confirm(): void {
    this.result.next(true);
    this.bsModalRef.hide();
  }

  decline(): void {
    this.result.next(false);
    this.bsModalRef.hide();
  }
}