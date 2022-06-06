import { GoogleMapsAPIWrapper } from '@agm/core';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';
import { User, Location } from '../Shared/interfaces';
import { AgmMap, MouseEvent, MapsAPILoader } from '@agm/core';
import { Config } from '../config';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: User;
  folderName: string = "UserPhotos";
  location: Location;

  constructor(private tokenService: TokenService, private authService: AuthService) { }

  ngOnInit(): void {
    this.location = {
      latitude: 0,
      longitude: 0
    }

    this.getUser();

  }

  addMarker(event) {
    this.user.location = {
      latitude: event.coords.lat,
      longitude: event.coords.lng
    };

    console.log(event);
  }

  getLocation() {
    if (!this.user.location) {
      navigator.geolocation.getCurrentPosition(position => {
        this.location = {
          latitude: position.coords.latitude,
          longitude: position.coords.longitude
        }
      });
    }
    else {
      this.location = this.user.location;
    }
  }

  getUser() {
    this.authService.getUser().subscribe(user => {
      this.user = user;
      this.getLocation();
    })
  }

  saveUser() {
    this.authService.updateUser(this.user)
      .subscribe();
  }

  createImgPath = (serverPath: string) => {
    return `${Config.apiUrl}/${serverPath}`;
  }

  imgPathChanged(event) {
    this.user.photoPath = event;
  }
}
