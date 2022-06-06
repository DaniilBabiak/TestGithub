import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';
import { UserChallengeService } from '../services/userchallenge.service';
import { NgbdSortableHeader } from '../Shared/classes';
import { PaginatedResult, SortConfig, SortEvent, User, UserChallenge } from '../Shared/interfaces';

@Component({
  selector: 'app-user-challenges',
  templateUrl: './user-challenges.component.html',
  styleUrls: ['./user-challenges.component.css']
})
export class UserChallengesComponent implements OnInit {
  userChallenges: UserChallenge[] = [];
  config: any;
  sortConfig: SortConfig;

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader<UserChallenge>>;

  constructor(private userChallengeService: UserChallengeService) { }

  ngOnInit(): void {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.sortConfig = {
      sortBy: 'Status',
      isSortAscending: true
    }

    this.getUserChallenges();
  }

  onSort({column, direction}: SortEvent<UserChallenge>) {

    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    if (direction === '' || column === '') {
      this.sortConfig.sortBy = 'Challenge.Name';
      this.sortConfig.isSortAscending = true;
    } else {
      this.sortConfig.sortBy = column;
      this.sortConfig.isSortAscending = direction !== 'desc';
    }
    this.getUserChallenges();
  }

  pageChanged(event) {
    this.config.currentPage = event;
    this.getUserChallenges();
  }

  getUserChallenges(): void {
    this.userChallengeService.getUserChallenges(this.config.currentPage, this.config.itemsPerPage, this.sortConfig)
    .subscribe((response: PaginatedResult<UserChallenge[]>) =>{
      this.userChallenges = response.resut;
      this.config = {
        itemsPerPage: 5,
        currentPage: response.pagination.currentPage,
        totalItems: response.pagination.totalItems
      };
    }
    );
  }
}
