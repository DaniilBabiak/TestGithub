import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VenueService } from '../services/venue.service';
import { Alert, Venue } from '../Shared/interfaces';
import { Location as Geolocation } from '../Shared/interfaces';
import { Location } from '@angular/common';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { NgbDatepickerConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-venue-details',
  templateUrl: './venue-details.component.html',
  styleUrls: ['./venue-details.component.css']
})
export class VenueDetailsComponent implements OnInit {
  venue: Venue;
  alerts: Alert[] = [];
  date: NgbDate | null;
  geolocation: Geolocation
  canDelete: boolean = false;

  constructor(private route: ActivatedRoute, private venueService: VenueService,
    private location: Location, private tokenService: TokenService,
    private config: NgbDatepickerConfig) {
    const current = new Date();
    this.config.minDate = {
      year: current.getFullYear(), month:
        current.getMonth() + 1, day: current.getDate()
    };
    this.config.outsideDays = 'hidden';
  }

  ngOnInit(): void {
    this.getLocation();
    this.getVenue();
  }

  getDate(venue: Venue) {
    var date = new Date(venue.date);
    this.date = new NgbDate(date.getFullYear(), date.getMonth() + 1, date.getDate())
  }

  getVenue(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    if (id != "0") {
      this.venueService.getVenue(id)
        .subscribe(venue => {
          this.venue = venue;
          var user = this.tokenService.getUser();
          if (this.venue.creator.userName == user.name) {
            this.canDelete = true;
          }
          this.getDate(venue);
        })
    }
    else {
      this.venue = {
        attendees: [],
        creator: null,
        date: null,
        description: null,
        id: null,
        location: null,
        title: null
      }
    }
  }

  isAssigned(): boolean {
    var user = this.venue.attendees.find(a => a.userName == this.tokenService.getUser().name);

    if (user) {
      return true;
    }
    else {
      return false;
    }
  }

  assignUser() {
    this.venueService.assignUser(this.venue.id).subscribe(() => this.goBack());
  }

  unassignUser() {
    this.venueService.unassignUser(this.venue.id).subscribe(() => this.goBack());
  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }
  goBack(): void {
    this.location.back();
  }

  save(): void {
    if (this.venue) {
      if (!this.venue.location) {
        this.alerts.push({ type: 'warning', message: 'Choose location' });
        return;
      }

      if (!this.venue.date) {
        this.alerts.push({ type: 'warning', message: 'Choose date' });
        return;
      }

      if (!this.venue.title) {
        this.alerts.push({ type: 'warning', message: 'Input title' });
        return;
      }

      if (!this.venue.description) {
        this.alerts.push({ type: 'warning', message: 'Input description' });
        return;
      }

      this.venueService.createVenue(this.venue)
        .subscribe(() => this.goBack());
    }
  }

  delete() {
    this.venueService.deleteVenue(this.venue.id).subscribe(() => this.goBack());
  }

  onDateSelection(date: NgbDate) {
    this.date = date;
    this.venue.date = new Date(this.date.year, this.date.month - 1, this.date.day)

  }

  closeAlert(alert: Alert) {
    this.alerts = this.alerts.filter(a => a != alert);
  }

  addMarker(event) {
    this.venue.location = {
      latitude: event.coords.lat,
      longitude: event.coords.lng
    };
  }

  getLocation() {
    navigator.geolocation.getCurrentPosition(position => {
      this.geolocation = {
        latitude: position.coords.latitude,
        longitude: position.coords.longitude
      }
    });
  }

}
