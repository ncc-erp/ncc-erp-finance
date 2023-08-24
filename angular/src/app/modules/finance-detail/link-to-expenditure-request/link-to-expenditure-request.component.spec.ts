import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkToExpenditureRequestComponent } from './link-to-expenditure-request.component';

describe('LinkToExpenditureRequestComponent', () => {
  let component: LinkToExpenditureRequestComponent;
  let fixture: ComponentFixture<LinkToExpenditureRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkToExpenditureRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkToExpenditureRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
