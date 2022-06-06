import { Component, OnInit } from '@angular/core';
import { settings } from 'cluster';
import { AuthService } from '../services/auth.service';
import { Alert, NotificationSettings } from '../Shared/interfaces';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  notificationSettings: NotificationSettings
  alerts: Alert[] = [];

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.getNotificationSettings();
  }

  getNotificationSettings() {
    this.authService.getNotificationSettings().subscribe(notificationSettings => {
      this.notificationSettings = notificationSettings;
    })
  }

  save() {
    this.authService.updateNotificationSettings(this.notificationSettings).subscribe(notificationSettings => {
      this.notificationSettings = notificationSettings;
      this.alerts.push({ type: 'success', message: 'Saved!' });
    })
  }

}
