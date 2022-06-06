import { not } from '@angular/compiler/src/output/output_ast';
import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { Config } from '../config';
import { Notification } from '../Shared/interfaces';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = Config.apiUrl;
  private hubConnection: signalR.HubConnection;
  private notificationsKey = 'notifications';

  public notifications: Notification[];
  public uncheckedCount: number = 0;

  constructor(private tokenService: TokenService) {
    this.notifications = this.getNotifications();
    this.uncheckedCount = this.notifications.filter(n => n.isChecked == false).length;
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.apiUrl + '/notification')
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public addNotificationListener() {
    var user = this.tokenService.getUser();
    var connection = 'transfernotification/' + user.id;
    this.hubConnection.on(connection, (data: Notification) => {
      this.saveNotification(data);
    });

    this.hubConnection.on('transfernotification/all', (data: Notification) => {
      this.saveNotification(data);
    });
  }

  private saveNotification(notification: Notification) {
    notification.isChecked = false;

    const notificationsJson = window.localStorage.getItem(this.notificationsKey);

    if (notificationsJson) {
      this.notifications = JSON.parse(notificationsJson) as Notification[];
    }
    this.notifications.push(notification);

    window.localStorage.removeItem(this.notificationsKey);
    window.localStorage.setItem(this.notificationsKey, JSON.stringify(this.notifications));
    this.uncheckedCount++;
  }

  public getNotifications(): Notification[] {
    const notifications = window.localStorage.getItem(this.notificationsKey);
    if (notifications) {
      return JSON.parse(notifications) as Notification[];
    }

    return [];
  }

  public checkNotification(notification: Notification) {
    this.notifications.find(n => n.id == notification.id).isChecked = true;
    window.localStorage.removeItem(this.notificationsKey);
    window.localStorage.setItem(this.notificationsKey, JSON.stringify(this.notifications));

    this.uncheckedCount--;
  }
}