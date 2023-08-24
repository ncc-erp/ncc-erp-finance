import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenuesTabComponent } from './revenues-tab.component';

describe('RevenuesTabComponent', () => {
  let component: RevenuesTabComponent;
  let fixture: ComponentFixture<RevenuesTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenuesTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenuesTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
