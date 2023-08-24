import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestChangeMainTabComponent } from './request-change-main-tab.component';

describe('RequestChangeMainTabComponent', () => {
  let component: RequestChangeMainTabComponent;
  let fixture: ComponentFixture<RequestChangeMainTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestChangeMainTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestChangeMainTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
