import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../services/notification.service';
import { TokenService } from '../services/token.service';
import { SettingsComponent } from '../settings/settings.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {

  title = 'UserClient';

  isExpanded = false;
  isLoggedIn = false;
  username?: string;
  coins?: number;
  showSetting = false;

  constructor(public notificationService: NotificationService, private tokenService: TokenService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenService.getToken();

    if (this.isLoggedIn) {
      const user = this.tokenService.getUser();

      this.username = user.name;
      this.coins = user.coins;

      this.notificationService.startConnection();
      this.notificationService.addNotificationListener();
    }
  }

  openSettings(){
    const modalRef = this.modalService.open(SettingsComponent);
    modalRef.componentInstance.name = 'World';
  }

  logout(): void {
    this.tokenService.signOut();
    window.location.reload();
  }

  collapse() {  
    this.isExpanded = false;  
  }  
  
  toggle() {  
    this.isExpanded = !this.isExpanded;  
  }  
  
  getNotificationCount() {  
    return this.notificationService.uncheckedCount;
  }

  getNotifications(){
    return this.notificationService.notifications;
  }

  checkNotification(notification){
    if(!notification.isChecked){
      this.notificationService.checkNotification(notification);
    }
  }

}
