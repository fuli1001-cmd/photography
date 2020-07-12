import { Component, OnInit } from '@angular/core';
import { SearchEventService } from 'src/app/services/search-event.service';

@Component({
  selector: 'search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css']
})
export class SearchBarComponent implements OnInit {

  searchKey: string;

  constructor(private searchEventService: SearchEventService) { }

  ngOnInit(): void {
  }

  onClickSearch(): void {
    if (this.searchKey)
      this.searchEventService.search(this.searchKey);
  }

  searchKeyChanged(): void {
    console.log("search key cleared");
    if (!this.searchKey)
      this.searchEventService.search(this.searchKey);
  }

}
