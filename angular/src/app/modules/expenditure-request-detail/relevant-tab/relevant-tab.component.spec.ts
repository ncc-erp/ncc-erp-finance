import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RelevantTabComponent } from './relevant-tab.component';

describe('RelevantTabComponent', () => {
  let component: RelevantTabComponent;
  let fixture: ComponentFixture<RelevantTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RelevantTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RelevantTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
