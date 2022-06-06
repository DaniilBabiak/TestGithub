import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AchievementService } from '../services/achievement.service';
import { NgbdSortableHeader } from '../Shared/classes';
import { Achievement, PaginatedResult, SortConfig, SortEvent } from '../Shared/interfaces';

@Component({
  selector: 'app-achievements',
  templateUrl: './achievements.component.html',
  styleUrls: ['./achievements.component.css']
})
export class AchievementsComponent implements OnInit {
  achievements: Achievement[] = [];
  config: any;
  sortConfig: SortConfig;

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader<Achievement>>;
  constructor(private route: ActivatedRoute, private achievementService: AchievementService, private router: Router) { }

  ngOnInit() {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.sortConfig = {
      sortBy: 'Name',
      isSortAscending: true
    }

    this.readQuery();
    this.changeQuery();

    this.getAchievements();
  }

  onSort({ column, direction }: SortEvent<Achievement>) {

    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    if (direction === '' || column === '') {
      this.sortConfig.sortBy = 'Name';
      this.sortConfig.isSortAscending = true;
    } else {
      this.sortConfig.sortBy = column;
      this.sortConfig.isSortAscending = direction !== 'desc';
    }
    this.getAchievements();
  }

  pageChanged(event) {
    this.config.currentPage = event;

    this.getAchievements();
    this.changeQuery();
  }

  getAchievements(): void {
    this.achievementService.getAchievements(this.config.currentPage, this.config.itemsPerPage, this.sortConfig)
      .subscribe((response: PaginatedResult<Achievement[]>) => {
        this.achievements = response.resut;
        this.config = {
          itemsPerPage: 5,
          currentPage: response.pagination.currentPage,
          totalItems: response.pagination.totalItems
        };
      }
      );
  }

  changeQuery() {
    const queryParams: Params = { currentPage: this.config.currentPage };

    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams: queryParams,
        queryParamsHandling: 'merge',
      });
  }

  readQuery() {
    this.route.queryParams.subscribe(params => {
      var currentPage = params['currentPage'];
      if (currentPage) {
        this.config.currentPage = parseInt(params['currentPage']);
      }
    })
  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }

}
