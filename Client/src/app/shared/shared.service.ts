import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { Observable } from 'rxjs';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { ConfirmDialogComponent } from './components/modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  bsModalRef?: BsModalRef;                                    
  displayingExpiringSessionModal = false;

  constructor(private modalService: BsModalService) { }

  showNotification(isSuccess: boolean, title: string, message: string) {
    const initalState: ModalOptions = {
      initialState: {
        isSuccess,
        title,
        message
      }
    };

    this.bsModalRef = this.modalService.show(NotificationComponent, initalState);
  }

  showConfirmation(title: string, message: string): Observable<boolean> {
    const config: ModalOptions = {
      initialState: {
        title: title,
        message: message,
        btnOkText: 'Delete',  
        btnCancelText: 'Cancel'
      },
      class: 'modal-md' 
    };
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);
    return this.bsModalRef.content!.result.asObservable();
  }

}
