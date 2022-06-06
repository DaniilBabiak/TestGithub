import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { VenueService } from '../services/venue.service';
import { PaginatedResult, User, Venue } from '../Shared/interfaces';
import { Location } from '../Shared/interfaces';

@Component({
  selector: 'app-venues',
  templateUrl: './venues.component.html',
  styleUrls: ['./venues.component.css']
})
export class VenuesComponent implements OnInit {
  allVenues: Venue[];
  nearestVenues: Venue[];
  config: any;
  user: User;
  location: Location;

  constructor(private venueService: VenueService, private authService: AuthService) { }

  ngOnInit(): void {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.getAllVenues();
    this.getNearestVenues();
    this.getUser();
  }

  pageChanged(event) {
    this.config.currentPage = event;

    this.getAllVenues();
  }

  getUser() {
    this.authService.getUser().subscribe(user => {
      this.user = user;
      this.getLocation();
    })
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

  getAllVenues(): void {
    this.venueService.getPagedVenues(this.config.currentPage, this.config.itemsPerPage)
      .subscribe((response: PaginatedResult<Venue[]>) => {
        this.allVenues = response.resut;
        this.config = {
          itemsPerPage: 5,
          currentPage: response.pagination.currentPage,
          totalItems: response.pagination.totalItems
        };
      }
      );
  }

  getNearestVenues(): void {
    this.venueService.getNearestVenues()
      .subscribe((response: Venue[]) => {
        this.nearestVenues = response;
      })
  }

}
