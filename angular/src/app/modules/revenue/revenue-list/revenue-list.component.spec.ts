import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueListComponent } from './revenue-list.component';

describe('RevenueListComponent', () => {
  let component: RevenueListComponent;
  let fixture: ComponentFixture<RevenueListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
