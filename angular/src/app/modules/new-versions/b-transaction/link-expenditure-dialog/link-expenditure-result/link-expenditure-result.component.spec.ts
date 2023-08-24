import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkExpenditureResultComponent } from './link-expenditure-result.component';

describe('LinkExpenditureResultComponent', () => {
  let component: LinkExpenditureResultComponent;
  let fixture: ComponentFixture<LinkExpenditureResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkExpenditureResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkExpenditureResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
